import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, NgForm, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-agenda-modal',
  templateUrl: './agenda-modal.component.html',
  styleUrls: ['./agenda-modal.component.scss']
})
export class AgendaModalComponent implements OnInit {
  @Input() session: any;
  @Output() saveAgenda = new EventEmitter();
  tasksForm: FormGroup;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.createTaskForm();
  }

  createTaskForm() {
    this.tasksForm = new FormGroup({
      tasks: new FormControl()
    });
  }

  updateAgenda(session) {
    const tasks = this.tasksForm.value.tasks;
    if (tasks !== null) {
      session.tasks = tasks;
      this.saveAgenda.emit(session);
    }
    this.activeModal.dismiss();
  }

}
