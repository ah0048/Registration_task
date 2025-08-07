import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HomeDto } from '../../models/home-models/home-dto';
import { HomeService } from '../../Services/home.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  homeData: HomeDto = {
    companyNameEn: '',
    logoUrl: undefined,
  };
  isLoading = true;

  constructor(private homeService: HomeService, private router: Router) {}

  ngOnInit(): void {
    // Check if user is authenticated
    const token = localStorage.getItem('token');
    if (!token) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.loadHomeData();
  }

  private loadHomeData(): void {
    this.homeService.getHomeData().subscribe({
      next: (result) => {
        this.isLoading = false;
        if (result.isSuccess && result.data) {
          this.homeData = {
            companyNameEn: result.data.companyNameEn || '',
            logoUrl: result.data.logoUrl,
          };
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: result.errorMessage || 'Failed to load home data.',
          });
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Home data error:', err);

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

  logout() {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will be logged out of your account.',
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, logout',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('otpCode');

        Swal.fire({
          title: 'Logged Out!',
          text: 'You have been successfully logged out.',
          icon: 'success',
          timer: 1500,
          showConfirmButton: false,
        }).then(() => {
          this.router.navigateByUrl('/login');
        });
      }
    });
  }

  onImageError(event: any): void {
    console.log('Image failed to load, hiding logo');
    this.homeData.logoUrl = undefined;
  }
}
