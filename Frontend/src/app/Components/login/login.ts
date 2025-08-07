import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { LoginDto } from '../../models/auth-models/login-dto';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginForm = new FormGroup({
    Email: new FormControl('', [Validators.required, Validators.email]),
    Password: new FormControl('', [
      Validators.required,
      Validators.minLength(7),
    ]),
  });

  constructor(private authService: AuthService, private router: Router) {}

  get getEmail() {
    return this.loginForm.controls['Email'];
  }
  get getPassword() {
    return this.loginForm.controls['Password'];
  }

  submit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    const formValue = this.loginForm.value;
    const dto: LoginDto = {
      email: formValue.Email ?? '',
      password: formValue.Password ?? '',
    };
    this.authService.login(dto).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          localStorage.setItem('token', result.data ?? '');
          Swal.fire({
            title: 'Success!',
            icon: 'success',
            text: 'successful login',
            draggable: true,
          }).then(() => {
            this.router.navigateByUrl('/home');
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: result.errorMessage || 'An error occurred during login.',
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
}
