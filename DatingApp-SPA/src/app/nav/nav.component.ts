import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(public authService: AuthService, private Alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
   // this.authService.currentPhotoUrl.subscribe(this.photoUrl = photoUrl);
   this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
   
  }

  login() {
   this.authService.login(this.model).subscribe(next => {
     this.Alertify.success('Logged in successfully');
     this.model = {};
   }, error => {
    this.Alertify.error(error);
   }, () => {
     this.router.navigate(['/members']);

   });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.Alertify.message('logged out');
    this.router.navigate(['']);
    // .navigate(['/members']

  }
}
