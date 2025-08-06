export interface Result<T = any> {
  isSuccess: boolean;
  data?: T;
  errorMessage?: string;
  errors?: string[];
}

export interface SimpleResult {
  isSuccess: boolean;
  errorMessage?: string;
  errors?: string[];
}
