import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IBeer, Beer} from "../../models/ontap.models.ts";
import {BeerService} from "./beers.service.ts";

@ng.Component({
    selector: 'beers',
    providers: [BeerService],
  template: require('./beers.html')
})
export class Beers {
    public beers: IBeer[];
    public editing: IBeer;
    public adding: IBeer;
    public errorMessage: any;

    constructor(private beerService: BeerService) {
        this.getBeers();
    }

    getBeers() {
        this.beerService.getBeers()
            .subscribe(
            cities => this.beers = cities,
            error => this.errorMessage = <any>error);
    }
    addBeer() {
        if (!this.adding) { return; }
        this.beerService.addBeer(this.adding)
            .subscribe(
                beer => {
                    this.beers.push(beer);
                    this.adding = null;
                },
            error => this.errorMessage = <any>error);
    }

    editCity(id) {
        const beer = new List(this.beers).Where(c => c.id === id).First();
        this.editing = new Beer(beer);
    }

    cancelEdit() {
        this.editing = null;
        this.adding = null;
    }

    saveCity() {
        if (!this.editing) { return; }
        this.beerService.changeBeer(this.editing)
            .subscribe(
            beer => {
                for (let c of this.beers) {
                    if (c.id === beer.id) {
                        c.name = beer.name;
                        c.description = beer.description;
                    }
                }
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }
     
    startAdd() {
        this.adding = new Beer({
            id: 'id',
            name: 'name',
            description: 'description',
            brewery: 'brewery',
            type: 'Lager',
            alcohol: 2.5,
            ibu: 30,
            gravity: 15,
        });
    }
}