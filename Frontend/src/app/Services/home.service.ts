import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HomeDto } from '../models/home-models/home-dto';
import { Observable } from 'rxjs';
import { Result } from '../models/shared/result';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  baseUrl: string = 'https://localhost:7270/api/Home';

  constructor(private http: HttpClient) {}

  getHomeData(): Observable<Result<HomeDto>> {
    return this.http.get<Result<HomeDto>>(`${this.baseUrl}`);
  }
}
