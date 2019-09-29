import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-grade-list',
  templateUrl: './grade-list.component.html',
  styleUrls: ['./grade-list.component.css']
})
export class GradeListComponent implements OnInit {
  @Input() userGrades: any = [];
  @Input() evals: any = [];
  @Input() showTable: boolean;
  @Output() toggleView = new EventEmitter<boolean>();
  selectedEval: any;
  colIndex: number;

  searchControl: FormControl = new FormControl();
  filteredUserGrades: any = [];


  constructor(private evalService: EvaluationService) { }

  ngOnInit() {

  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredUserGrades = [...this.userGrades];
    }

    const columns = Object.keys(this.userGrades[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.userGrades.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        // console.log(d[column]);
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredUserGrades = rows;
  }

  enterGrades(evaluation, userGrades, index) {
    this.evalService.setCurrentCurrentEval(evaluation, userGrades);
    this.evalService.setColIndex(index);
    this.toggleView.emit();
  }

}
