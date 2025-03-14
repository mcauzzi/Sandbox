import { HttpInterceptorFn } from '@angular/common/http';

export const jwtInterceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const jwtToken = localStorage.getItem('token');
  req = req.clone({
    setHeaders: {
      Authorization: `Bearer ${jwtToken}`,
    },
  });
  return next(req);
};
