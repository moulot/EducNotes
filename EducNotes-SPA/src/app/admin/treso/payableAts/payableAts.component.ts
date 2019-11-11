import { Component, OnInit } from '@angular/core';
import { PayableAt } from 'src/app/_models/payable-at';
import { ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TresoService } from 'src/app/_services/treso.service';

@Component({
  selector: 'app-payableats',
  templateUrl: './payableAts.component.html',
  styleUrls: ['./payableAts.component.scss'],
  animations: [SharedAnimations]
})
export class PayableAtsComponent implements OnInit {
  payableAts: PayableAt[];
  editField;
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private tresoService: TresoService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.payableAts = data.payableAts;
    });
  }

  changeValue(id: number, property: string, event: any) {
    const elementId = this.payableAts[id].id;
    this.editField = event.target.textContent;
  }

  updateList(id: number, property: string, event: any) {
      const elementId = this.payableAts[id].id;
    const editField = event.target.textContent;
    this.payableAts[id][property] = editField;
    const dataToSave = { name: this.payableAts[id].name, dayCount: this.payableAts[id].dayCount };
    if (this.editField && elementId) {
      this.tresoService.EditPayableAt(elementId, dataToSave).subscribe(() => {
        this.alertify.success('modificiation terminée');
      }, error => {
        this.alertify.error(error);
      });
    }
  }

}
