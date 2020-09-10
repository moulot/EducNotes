import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ClassLevel } from 'src/app/_models/classLevel';

@Component({
  selector: 'app-tuition-details',
  templateUrl: './tuition-details.component.html',
  styleUrls: ['./tuition-details.component.scss']
})
export class TuitionDetailsComponent implements OnInit {
  students: any;
  level: ClassLevel;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      const datalist = data['users'];
      this.students = datalist.students;
      this.level = datalist.level;
    });
  }

}
