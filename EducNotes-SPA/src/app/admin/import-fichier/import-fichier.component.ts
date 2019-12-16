import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { formatDate } from '@angular/common';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-import-fichier',
  templateUrl: './import-fichier.component.html',
  styleUrls: ['./import-fichier.component.scss']
})
export class ImportFichierComponent implements OnInit {
  fileToUpload: File = null;
  fichiers;
  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getAllFiles();
  }

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }

  postFile() {
    const formData: FormData = new FormData();
    formData.append('file', this.fileToUpload, this.fileToUpload.name);
    this.authService.saveFile(formData).subscribe(() => {
      this.alertify.success('enregistrement terminée..');
    }, error => {
      this.alertify.error(error);
    });

  }


  getAllFiles() {
    this.authService.getAllFiles().subscribe((res) => {
      this.fichiers = res;
    }, error => {
      console.log(error);
    });
  }

}
