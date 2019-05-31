import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 /**@Input() valuesFromHome: any; */ 
  @Output() cancelRegister = new EventEmitter();
  // model: any = {};
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>; // partial allows us to implement part of a class
  constructor(private authService: AuthService, private router: Router, 
    private alertify: AlertifyService, private fb: FormBuilder) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    },
  this.createRegisterForm();
  }
  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required],
    }, {validator: this.passwordMatchValidator});
  }
  passwordMatchValidator(g: FormGroup) {
    // compare two passwords if it matches, return null and if there is a mismatch return an object
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true};
  }
  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({},this.registerForm.value);//this clones values in regform to the annonymous object {}
      this.authService.register(this.user).subscribe(() => {
      this.alertify.success('Registration sucecessful');
      }, error => {
        this.alertify.error(error);

      }, () => {
        this.authService.login(this.user).subscribe(()=>{
          this.router.navigate(['/members']);
        });
      });
    }
//   this.authService.register(this.model).subscribe(() => {
//    this.alertify.success('registration was successful');
//   }, error => {
// this.alertify.error(error);
//   } );
console.log(this.registerForm.value);
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

}
