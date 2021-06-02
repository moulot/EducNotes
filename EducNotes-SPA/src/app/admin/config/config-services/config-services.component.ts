import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-config-services',
  templateUrl: './config-services.component.html',
  styleUrls: ['./config-services.component.scss']
})
export class ConfigServicesComponent implements OnInit {
  products: any;
  wait = false;

  constructor(private productService: ProductService, private router: Router) { }

  ngOnInit() {
    this.getProducts();
  }

  getProducts() {
    this.productService.getProducts().subscribe(data => {
      this.products = data;
      console.log(data);
    });
  }

  editerProduct(id) {
    this.router.navigate(['addService', id]);
  }

}
