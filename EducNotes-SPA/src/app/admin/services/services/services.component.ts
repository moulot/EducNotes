import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.scss']
})
export class ServicesComponent implements OnInit {
  services: any;
  wait = false;

  constructor(private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.services = data['services'];
      // console.log(this.services);
    });
  }

  // getProducts() {
  //   this.productService.getProducts().subscribe(data => {
  //     this.products = data;
  //     console.log(data);
  //   });
  // }

  editerService(id) {
    this.router.navigate(['addService', id]);
  }

}
