import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LoginViewModel } from '../Interfaces/LoginViewModel';
import { LoginResponseViewModel } from '../Interfaces/LoginResponseViewModel';
import { map } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(public Client:HttpClient) {}

  public login(userName:string,password:string){
     return this.Client.post<LoginResponseViewModel>(`${environment.baseAuthUrl}/login`,{username:userName,password:password}).pipe(map((res:LoginResponseViewModel)=>{
        localStorage.setItem('token',res.token);
        localStorage.setItem('refreshToken',res.refreshToken);
     }));
  }

  public logOut(){
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.Client.post(`${environment.baseAuthUrl}/logout`,{});
  }
}
