import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';

@Component({
  selector: 'app-level-recovery',
  templateUrl: './level-recovery.component.html',
  styleUrls: ['./level-recovery.component.scss']
})
export class LevelRecoveryComponent implements OnInit {
  parents: any;
  searchControl: FormControl = new FormControl();
  filteredParents: any[] = [];
  page = 1;
  pageSize = 10;
  recoveryForm: FormGroup;
  showCustomBtn = false;
  selectAllText = 'tout sélectionner';
  wait = false;
  className: string;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private commService: CommService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.createRecoveryForm();
    this.route.data.subscribe((data: any) => {
      this.parents = data['parents'];
      const cname = this.parents[0].children[0].className;
      this.className = cname != null ? cname : this.parents[0].children[0].levelName;
      this.addParentSelections();
      this.filteredParents = this.parents;
    });
    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  createRecoveryForm() {
    this.recoveryForm = this.fb.group({
      byEmail: [false],
      bySms: [false],
      selectedParents: this.fb.array([])
    }, {validators: this.formValidator});
  }

  formValidator(g: FormGroup) {
    let formNOK = false;
    let noneSelected = true;
    const selectedParents = g.get('selectedParents').value;

    for (let i = 0; i < selectedParents.length; i++) {
      const parent = selectedParents[i];
      if (parent.selected === true) {
        noneSelected = false;
        break;
      }
    }

    let commTypeSelected = false;
    const byEmail = g.get('byEmail').value;
    const bySms = g.get('bySms').value;
    if (byEmail !== false || bySms !== false) {
      commTypeSelected = true;
    }

    if (noneSelected === true || commTypeSelected === false) {
      formNOK = true;
    }

    return {'formNOK': formNOK};
  }

  addParentSelections(): void {
    const selectedParents = this.recoveryForm.get('selectedParents') as FormArray;
    this.parents.forEach(x => {
      selectedParents.push(this.fb.group({
        selected: false
      }));
    });
  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredParents = [...this.parents];
    }
    const columns = Object.keys(this.parents[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.parents.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredParents = rows;
  }

  selectAll(value) {
    let checked = false;
    if (value.checked) {
      checked = true;
      this.selectAllText = 'désélectionner tout';
    } else {
      this.selectAllText = 'sélectionner tout';
    }
    const selectedParents = this.recoveryForm.get('selectedParents') as FormArray;
    for (let i = 0; i < this.recoveryForm.value.selectedParents.length; i++) {
      const elt = this.recoveryForm.value.selectedParents[i];
      selectedParents.at(i).get('selected').setValue(checked);
    }
  }

  sendRecoveryComm() {
    this.wait = true;
    let parentsToFollowUp = [];
    const selectedParents = this.recoveryForm.get('selectedParents') as FormArray;
    for (let i = 0; i < this.recoveryForm.value.selectedParents.length; i++) {
      const elt = this.recoveryForm.value.selectedParents[i];
      if (elt.selected === true) {
        this.parents[i].byEmail = this.recoveryForm.value.byEmail;
        this.parents[i].bySms = this.recoveryForm.value.bySms;
        parentsToFollowUp = [...parentsToFollowUp, this.parents[i]];
      }
    }

    this.commService.sendRecoveryComm(parentsToFollowUp).subscribe(() => {
      this.alertify.success('les messages ont été envoyés avec succès');
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
