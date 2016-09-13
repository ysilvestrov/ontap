import * as ng from '@angular/core';
import {CORE_DIRECTIVES, FORM_DIRECTIVES, NgClass, NgIf} from '@angular/common';
import { Http } from '@angular/http';
import { List } from "../../modules/linq.ts";
import {IPub, IBeer, IServe, Serve} from "../../models/ontap.models.ts";
import {EPubService} from "../epubs/epubs.service.ts";
import {BeerService} from "../beers/beers.service.ts";
import {ServeService} from "./serves.service.ts";
import {AppComponent,AppService} from "../../modules/appComponent.ts";

@ng.Component({
    selector: 'serves',
    providers: [ServeService, EPubService, BeerService],
  template: require('./serves.html')
})
export class Serves extends  AppComponent<IServe, ServeService> {
    public pubs: IPub[];
    public beers: IBeer[];

    constructor(elmService: ServeService, private pubService: EPubService, private beerService: BeerService) {
        super(elmService);
        this.getPubs();
        this.getBeers();
    }

    public startAdd() {
        super.startAdd();
        this.adding.servedIn = new List(this.pubs).First();
        this.adding.served = new List(this.beers).First();
    }

    getPubs() {
        this.pubService.get()
            .subscribe(
            pubs => this.pubs = pubs,
            error => this.errorMessage = <any>error);
    }

    getBeers() {
        this.beerService.get()
            .subscribe(
            beers => this.beers = beers,
            error => this.errorMessage = <any>error);
    }

    onEditChangePub(id) {
        this.onChangePub(this.editing, id);
    }

    onAddChangePub(id) {
        this.onChangePub(this.adding, id);
    }

    onChangePub(obj: IServe, id:string) {
        obj.servedIn = new List(this.pubs).Where(c => c.id === id).First();
    }

    onEditChangeBeer(id) {
        this.onChangeBeer(this.editing, id);
    }

    onAddChangeBeer(id) {
        this.onChangeBeer(this.adding, id);
    }

    onChangeBeer(obj: IServe, id:string) {
        obj.served = new List(this.beers).Where(c => c.id === id).First();
    }
}