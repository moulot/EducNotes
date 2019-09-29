import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { BsModalRef } from 'ngx-bootstrap';

@Component({
  selector: 'app-add-children',
  templateUrl: './add-children.component.html',
  styleUrls: ['./add-children.component.scss']
})
export class AddChildrenComponent implements OnInit {
  children: User;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit() {
  }

}
