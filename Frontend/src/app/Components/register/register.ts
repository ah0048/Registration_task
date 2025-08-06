import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { RegisterDto } from '../../models/auth-models/register-dto';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, RouterLink, NgxSpinnerModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerForm = new FormGroup({
    companyNameAr: new FormControl('', [
      Validators.required,
      Validators.pattern(/^[\u0600-\u06FF\s]{2,100}$/),
      Validators.maxLength(50),
    ]),
    companyNameEn: new FormControl('', [
      Validators.required,
      Validators.pattern(/^[A-Za-z\s]{2,100}$/),
      Validators.maxLength(50),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('', [
      Validators.pattern(/^[\+]?[0-9\-\(\)\s]+$/),
    ]),
    websiteUrl: new FormControl('', [
      Validators.required,
      Validators.pattern(/^https?:\/\/.+/),
    ]),
    logo: new FormControl<File | null>(null),
  });

  constructor(
    private authService: AuthService,
    private router: Router,
    private spinner: NgxSpinnerService
  ) {}

  get getCompanyNameAr() {
    return this.registerForm.controls['companyNameAr'];
  }
  get getCompanyNameEn() {
    return this.registerForm.controls['companyNameEn'];
  }
  get getEmail() {
    return this.registerForm.controls['email'];
  }
  get getPhoneNumber() {
    return this.registerForm.controls['phoneNumber'];
  }
  get getWebsiteUrl() {
    return this.registerForm.controls['websiteUrl'];
  }
  get getLogo() {
    return this.registerForm.controls['logo'];
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.registerForm.patchValue({
        logo: file,
      });
    }
  }

  submit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const formValue = this.registerForm.value;
    const dto: RegisterDto = {
      companyNameAr: formValue.companyNameAr!,
      companyNameEn: formValue.companyNameEn!,
      email: formValue.email!,
      websiteUrl: formValue.websiteUrl!,
      ...(formValue.phoneNumber && { phoneNumber: formValue.phoneNumber }),
      ...(formValue.logo && { logo: formValue.logo }),
    };

    this.spinner.show();
    this.authService.register(dto).subscribe({
      next: (result) => {
        this.spinner.hide();
        if (result.isSuccess) {
          localStorage.setItem('userId', result.data!.id);
          localStorage.setItem('otpCode', result.data!.otpCode);
          Swal.fire({
            title: 'Success!',
            icon: 'success',
            text: 'Registration was successful! An OTP was sent to your email.',
            draggable: true,
          }).then(() => {
            this.router.navigateByUrl('/check-otp');
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Registration Failed',
            text:
              result.errorMessage || 'An error occurred during registration.',
          });
        }
      },
      error: (err) => {
        this.spinner.hide();
        console.error('Registration error:', err);

        let errorMessage = 'An unexpected error occurred. Please try again.';
        if (err.error?.errorMessage) {
          errorMessage = err.error.errorMessage;
        } else if (err.error?.errors?.length > 0) {
          errorMessage = err.error.errors.join(', ');
        }

        Swal.fire({
          icon: 'error',
          title: 'Registration Failed',
          text: errorMessage,
        });
      },
    });
  }
}
