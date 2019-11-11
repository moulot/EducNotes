import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Periodicity } from 'src/app/_models/periodicity';
import { ActivatedRoute } from '@angular/router';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-periodicities',
  templateUrl: './periodicities.component.html',
  styleUrls: ['./periodicities.component.scss'],
  animations: [SharedAnimations]
})
export class PeriodicitiesComponent implements OnInit {
  periodicities: Periodicity[];
editField: string;

  constructor(private route: ActivatedRoute, private tresoService: TresoService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.periodicities = data.periodicities;
    });
  }

  changeValue(id: number, property: string, event: any) {
    const elementId = this.periodicities[id].id;
    this.editField = event.target.textContent;
  }

  updateList(id: number, property: string, event: any) {
    const elementId = this.periodicities[id].id;
    const editField = event.target.textContent;
    this.periodicities[id][property] = editField;
    if (this.editField && elementId) {
      this.tresoService.EditPeriodicity(elementId, this.editField).subscribe(() => {
        this.alertify.success('modificiation terminée');
      }, error => {
        console.log(error);
      });
    }
  }


}
