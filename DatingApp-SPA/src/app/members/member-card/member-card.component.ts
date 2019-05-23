import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
// we want to pass our user to this component so us @input because its user from parrent to child
@Input() user: User;
  constructor() { }

  ngOnInit() {
  }

}
