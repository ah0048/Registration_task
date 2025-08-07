import { Component } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';
import { SetPasswordDto } from '../../models/auth-models/set-password-dto';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-set-password',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './set-password.html',
  styleUrl: './set-password.css',
})
export class SetPassword {
  userId: string = localStorage.getItem('userId') ?? '';
  constructor(private authService: AuthService, private router: Router) {}

  setPasswordForm = new FormGroup(
    {
      Password: new FormControl('', [
        Validators.required,
        Validators.minLength(7),
      ]),
      ConfirmPassword: new FormControl('', [Validators.required]),
    },
    {
      validators: (control: AbstractControl) => {
        return this.passwordsMatch(control as FormGroup);
      },
    }
  );

  private passwordsMatch(group: FormGroup) {
    const pass = group.get('Password')?.value;
    const confirm = group.get('ConfirmPassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  }

  get getPassword() {
    return this.setPasswordForm.controls['Password'];
  }
  get getConfirmPassword() {
    return this.setPasswordForm.controls['ConfirmPassword'];
  }

  submit() {
    if (this.setPasswordForm.invalid) {
      this.setPasswordForm.markAllAsTouched();
      return;
    }
    const formValue = this.setPasswordForm.value;
    const dto: SetPasswordDto = {
      id: localStorage.getItem('userId') ?? '',
      newPassword: formValue.Password ?? '',
      confirmPassword: formValue.ConfirmPassword ?? '',
    };

    this.authService.setPassword(dto).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          Swal.fire({
            title: 'Success!',
            icon: 'success',
            text: result.data || 'Password was set successfully',
            draggable: true,
          }).then(() => {
            this.router.navigateByUrl('/login');
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Error',
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
          title: 'Error',
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
          Swal.fire({
            title: 'Success!',
            icon: 'success',
            text: 'A new OTP has been sent to your email.',
            draggable: true,
          }).then(() => {
            this.router.navigateByUrl('/check-otp');
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
