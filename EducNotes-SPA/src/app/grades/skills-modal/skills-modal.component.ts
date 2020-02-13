import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MDBModalRef } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-skills-modal',
  templateUrl: './skills-modal.component.html',
  styleUrls: ['./skills-modal.component.css']
})
export class SkillsModalComponent implements OnInit {
  @Output() updateProgElt = new EventEmitter();
  // @Input() coursesSkills: any;
  // @Input() title: string;
  // @Input() subtitle: string;
  courseSkills: any;
  progElts: any[];
  progEltsHeader: string;

  constructor(public modalRef: MDBModalRef) { }

  ngOnInit() {
  }

  loadProgElts(data) {
    this.progElts = [];
    this.progElts = data;
  }

  addProgElt(e, progElt) {
    console.log(progElt);
    if (e) {
      progElt.checked = true;
    } else {
      progElt.checked = false;
    }
    this.updateProgElt.emit(progElt);
  }

}
