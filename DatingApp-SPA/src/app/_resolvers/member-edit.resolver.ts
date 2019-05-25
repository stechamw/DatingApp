import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()

export class MemberEditResolver implements Resolve<User> /** resolve to user */{
    /**constructor to inject user service */
    constructor(private userService: UserService, private authService: AuthService,
        private router: Router, private alertify: AlertifyService){}

        resolve(route: ActivatedRouteSnapshot) : Observable<User>{
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);/**in rjx6 this returns null observable*/
            })
        );
        }
}