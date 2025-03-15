import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './Auth/Services/auth-service.service';
import { LoginComponent } from "./login/login.component";
import { CommonModule } from '@angular/common';
import { MenuModule } from 'primeng/menu';
import { RoutingMenuItem } from './Interfaces/routing-menu-item';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { PanelModule } from 'primeng/panel';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers: [AuthService],
  imports: [LoginComponent,RouterOutlet,CommonModule,MenuModule,ScrollPanelModule,PanelModule]
})
export class AppComponent {
  title = 'WeatherWebApiFrontend';
  get isLoggedIn(){
    return this.auth.isLoggedIn;
  }
  routingItems: RoutingMenuItem[];
  constructor(public auth:AuthService,public router:Router){
    this.routingItems = this.router.config.map((route):RoutingMenuItem=>{return {label:route.title?.toString()??"",path:route.path??""}});
  }
}
