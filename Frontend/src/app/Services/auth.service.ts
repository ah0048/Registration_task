import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterDto } from '../models/auth-models/register-dto';
import { Observable } from 'rxjs';
import { RegisterResultDto } from '../models/auth-models/register-result-dto';
import { CheckOtpDto } from '../models/auth-models/check-otp-dto';
import { SetPasswordDto } from '../models/auth-models/set-password-dto';
import { LoginDto } from '../models/auth-models/login-dto';
import { Result, SimpleResult } from '../models/shared/result';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl: string = 'https://localhost:7270/api/Auth';

  constructor(private http: HttpClient) {}

  register(dto: RegisterDto): Observable<Result<RegisterResultDto>> {
    const formData = new FormData();
    formData.append('companyNameAr', dto.companyNameAr);
    formData.append('companyNameEn', dto.companyNameEn);
    formData.append('email', dto.email);
    formData.append('websiteUrl', dto.websiteUrl);

    if (dto.phoneNumber) {
      formData.append('phoneNumber', dto.phoneNumber);
    }

    if (dto.logo) {
      formData.append('logo', dto.logo);
    }

    return this.http.post<Result<RegisterResultDto>>(
      `${this.baseUrl}/register`,
      formData
    );
  }

  checkOtp(dto: CheckOtpDto): Observable<Result<boolean>> {
    return this.http.post<Result<boolean>>(`${this.baseUrl}/otp-valid`, dto);
  }

  resendOtp(userId: string): Observable<Result<RegisterResultDto>> {
    return this.http.get<Result<RegisterResultDto>>(
      `${this.baseUrl}/resend-otp/${userId}`
    );
  }

  setPassword(dto: SetPasswordDto): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.baseUrl}/set-password`, dto);
  }

  login(dto: LoginDto): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.baseUrl}/login`, dto);
  }
}
