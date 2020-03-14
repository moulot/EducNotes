import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, NgForm, FormControl } from '@angular/forms';

@Component({
  selector: 'app-agenda-modal',
  templateUrl: './agenda-modal.component.html',
  styleUrls: ['./agenda-modal.component.scss']
})
export class AgendaModalComponent implements OnInit {
  @Input() session: any;
  @Output() saveAgenda = new EventEmitter();
  tasksForm: FormGroup;

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder) { }

  ngOnInit() {
    this.createTaskForm();
  }

  createTaskForm() {
    this.tasksForm = new FormGroup({
      tasks: new FormControl()
    });
  }

  updateAgenda(session) {
    let tasks = this.tasksForm.value.tasks;
    if (tasks === null) {
      tasks = session.tasks;
    }
    session.tasks = tasks;
    this.saveAgenda.emit(session);
    this.activeModal.dismiss();
  }

}
