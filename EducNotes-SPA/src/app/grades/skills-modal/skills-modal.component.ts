import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-skills-modal',
  templateUrl: './skills-modal.component.html',
  styleUrls: ['./skills-modal.component.css']
})
export class SkillsModalComponent implements OnInit {
  @Input() title: string;
  @Input() subtitle: string;
  @Input() coursesSkills = [];
  @Output() updateProgElt = new EventEmitter();
  progElts: any[];
  progEltsHeader: string;

  constructor() { }

  ngOnInit() {
  }

  loadProgElts(data, skillName) {
    this.progEltsHeader = skillName;
    this.progElts = [];
    this.progElts = data;
  }

  addProgElt(e, progElt) {
    if (e) {
      progElt.checked = true;
    } else {
      progElt.checked = false;
    }
    this.updateProgElt.emit(progElt);
  }

}
