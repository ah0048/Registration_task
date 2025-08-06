import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';
import { CheckOtpDto } from '../../models/auth-models/check-otp-dto';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-check-otp',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './check-otp.html',
  styleUrl: './check-otp.css',
})
export class CheckOtp {
  otpToolTip: string = localStorage.getItem('otpCode') ?? '';
  userId: string = localStorage.getItem('userId') ?? '';
  checkOtpForm = new FormGroup({
    otpCode: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{6}$/),
    ]),
  });

  constructor(private authService: AuthService, private router: Router) {}

  get getOtpCode() {
    return this.checkOtpForm.controls['otpCode'];
  }

  submit() {
    if (this.checkOtpForm.invalid) {
      this.checkOtpForm.markAllAsTouched();
      return;
    }
    const formValue = this.checkOtpForm.value;
    const dto: CheckOtpDto = {
      id: localStorage.getItem('userId') ?? '',
      otpCode: formValue.otpCode ?? '',
    };

    this.authService.checkOtp(dto).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          if (result.data) {
            Swal.fire({
              title: 'Success!',
              icon: 'success',
              text: 'you can now set your password!',
              draggable: true,
            }).then(() => {
              this.router.navigateByUrl('/set-password');
            });
          } else {
            Swal.fire({
              icon: 'error',
              title: 'Expired OTP',
              text: 'Your OTP has expired you need to request a new one!',
            });
          }
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Invalid OTP',
            text:
              result.errorMessage || 'An error occurred during registration.',
          });
        }
      },
      error: (err) => {
        console.error('otp check error:', err);

        let errorMessage = 'An unexpected error occurred. Please try again.';
        if (err.error?.errorMessage) {
          errorMessage = err.error.errorMessage;
        } else if (err.error?.errors?.length > 0) {
          errorMessage = err.error.errors.join(', ');
        }
        Swal.fire({
          icon: 'error',
          title: 'OTP check Failed',
          text: errorMessage,
        });
      },
    });
  }

  resendOTP() {
    this.authService.resendOtp(this.userId).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          localStorage.setItem('userId', result.data!.id);
          localStorage.setItem('otpCode', result.data!.otpCode);
          // Update the tooltip with the new OTP
          this.otpToolTip = result.data!.otpCode;
          Swal.fire({
            title: 'Success!',
            icon: 'success',
            text: 'A new OTP has been sent to your email.',
            draggable: true,
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Failed to resend OTP',
            text:
              result.errorMessage || 'An error occurred during registration.',
          });
        }
      },
      error: (err) => {
        console.error('otp check error:', err);

        let errorMessage = 'An unexpected error occurred. Please try again.';
        if (err.error?.errorMessage) {
          errorMessage = err.error.errorMessage;
        } else if (err.error?.errors?.length > 0) {
          errorMessage = err.error.errors.join(', ');
        }
        Swal.fire({
          icon: 'error',
          title: 'OTP Resend Failed',
          text: errorMessage,
        });
      },
    });
  }
}
