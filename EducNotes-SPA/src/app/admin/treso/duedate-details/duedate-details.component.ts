import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-duedate-details',
  templateUrl: './duedate-details.component.html',
  styleUrls: ['./duedate-details.component.scss']
})
export class DuedateDetailsComponent implements OnInit {
  dueDate: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.dueDate = data['dueDate'];
    });
    console.log(this.dueDate);
  }

}
