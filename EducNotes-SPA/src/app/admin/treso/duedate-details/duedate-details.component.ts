import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-duedate-details',
  templateUrl: './duedate-details.component.html',
  styleUrls: ['./duedate-details.component.scss']
})
export class DuedateDetailsComponent implements OnInit {
  dueDate: any;
  children: any;
  totalInvoiced = 0;
  totalPaid = 0;
  totalBalance = 0;
  searchControl: FormControl = new FormControl();
  filteredChildren: any[] = [];
  page = 1;
  pageSize = 10;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      const dueDateData = data['dueDate'];
      this.dueDate = dueDateData.date;
      this.children = dueDateData.children;
      this.filteredChildren = dueDateData.children;
      this.totalInvoiced = dueDateData.totalInvoiced;
      this.totalPaid = dueDateData.totalPaid;
      this.totalBalance = dueDateData.totalBalance;
    });
    console.log(this.children);
    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredChildren = [...this.children];
    }
    const columns = Object.keys(this.children[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.children.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredChildren = rows;
  }

}
