import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  photoUrl: string;
  @ViewChild('editForm') editForm: NgForm; //editForm is the reference variable declared in member-edit html
  @HostListener('window: beforeunload', ['$event'])
  unLoadNotification($event: any) {
    if(this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    
    this.route.data.subscribe(data => {
      this.user = data.user; /** assigning data from out route */
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl=>this.photoUrl = photoUrl)
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next=>{
      this.alertify.success('Profile updated successfully');
      this.editForm.reset(this.user);
    },error=>{
      this.alertify.error(error);
    });
  
  }

  UpdateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}
