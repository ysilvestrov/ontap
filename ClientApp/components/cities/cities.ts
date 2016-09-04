import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {ICity, City} from "../../models/ontap.models.ts";
import {CityService} from "./cities.service.ts";

@ng.Component({
    selector: 'cities',
    providers: [CityService],
  template: require('./cities.html')
})
export class Cities {
    public cities: ICity[];
    public editing: ICity;
    public adding: ICity;
    public errorMessage: any;

    constructor(private cityService: CityService) {
        this.getCities();
    }

    getCities() {
        this.cityService.getCities()
            .subscribe(
            cities => this.cities = cities,
            error => this.errorMessage = <any>error);
    }
    addCity() {
        if (!this.adding) { return; }
        this.cityService.addCity(this.adding)
            .subscribe(
                city => {
                    this.cities.push(city);
                    this.adding = null;
                },
            error => this.errorMessage = <any>error);
    }

    editCity(id) {
        const city = new List(this.cities).Where(c => c.id === id).First();
        this.editing = new City(city);
    }

    cancelEdit() {
        this.editing = null;
        this.adding = null;
    }

    saveCity() {
        if (!this.editing) { return; }
        this.cityService.changeCity(this.editing)
            .subscribe(
            city => {
                for (let c of this.cities) {
                    if (c.id === city.id) {
                        c.name = city.name;
                    }
                }
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }

    startAddCity() {
        this.adding = new City({id:'id', name:'name'});
    }
}