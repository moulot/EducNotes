import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/shared/services/product.service';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-inscription-panel',
  templateUrl: './inscription-panel.component.html',
  styleUrls: ['./inscription-panel.component.scss']
})
export class InscriptionPanelComponent implements OnInit {
  products$;
  levels;

  constructor(private productService: ProductService, private adminService: AdminService) { }

  ngOnInit() {
    // this.products$ = this.productService.getProducts();
    // this.getLevelsDetails();
  }
  // getLevelsDetails() {
  //   this.adminService.getLevelsInscDetails().subscribe((res) => {
  //     this.levels = res;
  //   }, error => {
  //     console.log(error);
  //   });
  // }
}
