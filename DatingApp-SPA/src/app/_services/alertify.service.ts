import { Injectable } from '@angular/core';
declare let alertify: any; /** we declare it for us to use it inside the service */

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }
confirm(message: string, okCallback: () => any) /** OK call back will be a function of type any */ {
  alertify.confirm(message, function(e) {
    if (e) {
      okCallback();
    } else {}
  });
}
success(message: string) {
  alertify.success(message);
}

error(message: string) {
  alertify.error(message);
}

warning(message: string) {
  alertify.warning(message);
}

message(message: string) {
  alertify.message(message);
}

}