import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, ICity, Pub} from "../../models/ontap.models.ts";
import {EPubService} from "./epubs.service.ts";
import {CityService} from "../cities/cities.service.ts";

@ng.Component({
    selector: 'epubs',
    providers: [EPubService, CityService],
  template: require('./epubs.html')
})
export class EPubs {
    public pubs: IPub[];
    public cities: ICity[];
    public editing: IPub;
    public adding: IPub;
    public errorMessage: any;

    constructor(private pubService: EPubService, private cityService: CityService) {
        this.getPubs();
        this.getCities();
    }

    getPubs() {
        this.pubService.get()
            .subscribe(
            pubs => this.pubs = pubs,
            error => this.errorMessage = <any>error);
    }
    getCities() {
        this.cityService.getCities()
            .subscribe(
            cities => this.cities = cities,
            error => this.errorMessage = <any>error);
    }
    addPub() {
        if (!this.adding) { return; }
        this.pubService.add(this.adding)
            .subscribe(
                pub => {
                    this.pubs.push(pub);
                    this.adding = null;
                },
            error => this.errorMessage = <any>error);
    }

    editPub(id) {
        const pub = new List(this.pubs).Where(c => c.id === id).First();
        this.editing = new Pub(pub);
    }

    cancelEdit() {
        this.editing = null;
        this.adding = null;
    }

    savePub() {
        if (!this.editing) { return; }
        this.pubService.change(this.editing)
            .subscribe(
            pub => {
                for (let c of this.pubs) {
                    if (c.id === pub.id) {
                        c.name = pub.name;
                        c.city = pub.city;
                        c.address = pub.address;
                        c.serves = pub.serves;
                    }
                }
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }
     
    startAdd() {
        this.adding = new Pub({
            id: 'id',
            name: 'name',
            address: 'address',
            city: this.cities[0],
            serves: [],
        });
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