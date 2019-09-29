import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-booknote',
  templateUrl: './booknote.component.html',
  styleUrls: ['./booknote.component.css']
})
export class BooknoteComponent implements OnInit {
  courses: any;
  @Input() user: User;
  photoUrl: string;

  constructor() { }

  ngOnInit() {
    // this.user = JSON.parse(localStorage.getItem('user'));
  }

}
