import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { idLocale } from 'ngx-bootstrap';

// take into account of the use of token headers for authorization
/** const httpOptions = {
headers: new HttpHeaders({
  'Authorization': 'Bearer ' + localStorage.getItem('token')
})
}; */


@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> /** it will return an observable of type array of users */{

  return this.http.get<User[]>(this.baseUrl + 'users'); // get will be returning a user array
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}
updateUser(id: number, user: User){
return this.http.put(this.baseUrl + 'users/' + id, user)
}
}
