import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ActivatedRoute } from '@angular/router';
import { Product } from 'src/app/_models/product';
import { SchoolServicesDto } from 'src/app/_models/school-services';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss'],
  animations: [SharedAnimations]
})
export class ProductsListComponent implements OnInit {
  products: any[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.products = data.products;
    });
  }

}
