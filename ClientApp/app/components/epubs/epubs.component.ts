import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, ICity, Pub} from "../../models/ontap.models.ts";
import {EPubService} from "./epubs.service.ts";
import {CityService} from "../cities/cities.service.ts";
import {AppComponent,AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'epubs',
    providers: [EPubService, CityService],
  template: require('./epubs.component.html')
})
export class EPubsComponent extends  AppComponent<IPub, EPubService> {
    public cities: ICity[];

    constructor(elmService: EPubService, private cityService: CityService) {
        super(elmService);
        this.getCities();
    }

    startAdd() {
        super.startAdd();
        this.adding.city = new List(this.cities).First();
    }

    getCities() {
        this.cityService.get()
            .subscribe(
            cities => this.cities = cities,
            error => this.errorMessage = <any>error);
    }

    onEditChangeCity(id) {
        this.onChangeCity(this.editing, id);
    }

    onAddChangeCity(id) {
        this.onChangeCity(this.adding, id);
    }

    onChangeCity(obj: IPub, id:string) {
        obj.city = new List(this.cities).Where(c => c.id === id).First();
    }
}