import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-comm',
  templateUrl: './comm.component.html',
  styleUrls: ['./comm.component.scss'],
  animations :  [SharedAnimations]
})
export class CommComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
