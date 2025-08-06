import { Routes } from '@angular/router';
import { Login } from './Components/login/login';
import { Register } from './Components/register/register';
import { Home } from './Components/home/home';
import { CheckOtp } from './Components/check-otp/check-otp';
import { SetPassword } from './Components/set-password/set-password';
import { authGuard } from './Guards/auth-guard';

export const routes: Routes = [
  { path: '', component: Home, canActivate: [authGuard] },
  { path: 'home', component: Home, canActivate: [authGuard] },
  { path: 'register', component: Register },
  { path: 'check-otp', component: CheckOtp },
  { path: 'set-password', component: SetPassword },
  { path: 'login', component: Login },
];
