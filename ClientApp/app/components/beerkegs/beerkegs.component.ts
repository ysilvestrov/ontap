import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import { IPub, IBeer, IBrewery, IKeg, IBeerKeg, BeerKeg, KegStatus } from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {BeerService} from "../beers/beers.service";
import {KegService} from "../kegs/kegs.service";
import {BreweryService} from "../breweries/breweries.service";
import {BeerKegService} from "./beerkegs.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'beerkegs',
    providers: [BeerKegService, EPubService, BeerService, KegService, BreweryService],
  template: require('./beerkegs.component.html')
})
export class BeerKegsComponent extends  AppComponent<IBeerKeg, BeerKegService> {
    public pubs: IPub[];
    public beers: IBeer[];
    public breweries: IBrewery[];
    public kegs: IKeg[];

// ReSharper disable InconsistentNaming
    public KegStatus: typeof KegStatus = KegStatus;
// ReSharper restore InconsistentNaming

    constructor(elmService: BeerKegService,
        private pubService: EPubService,
        private beerService: BeerService,
        private breweryService: BreweryService,
        private kegService: KegService,
        public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getPubs();
        this.getBeers();
        this.getBreweries();
        this.getKegs();
    }

    public startAdd() {
        super.startAdd();
        this.adding.buyer = new List(this.pubs).First();
        this.adding.owner = new List(this.breweries).First();
        this.adding.beer = new List(this.beers).First();
        this.adding.keg = new List(this.kegs).First();
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

    getBreweries() {
        this.breweryService.get()
            .subscribe(
            breweries => this.breweries = breweries,
            error => this.errorMessage = <any>error);
    }

    getKegs() {
        this.kegService.get()
            .subscribe(
            kegs => this.kegs = kegs,
            error => this.errorMessage = <any>error);
    }

    onEditChangeBuyer(id) {
        this.onChangeBuyer(this.editing, id);
    }

    onAddChangeBuyer(id) {
        this.onChangeBuyer(this.adding, id);
    }

    onChangeBuyer(obj: IBeerKeg, id:string) {
        obj.buyer = new List(this.pubs).Where(c => c.id === id).First();
    }

    onEditChangeOwner(id) {
        this.onChangeOwner(this.editing, id);
    }

    onAddChangeOwner(id) {
        this.onChangeOwner(this.adding, id);
    }

    onChangeOwner(obj: IBeerKeg, id:string) {
        obj.owner = new List(this.breweries).Where(c => c.id === id).First();
    }

    onEditChangeBeer(id) {
        this.onChangeBeer(this.editing, id);
    }

    onAddChangeBeer(id) {
        this.onChangeBeer(this.adding, id);
    }

    onChangeBeer(obj: IBeerKeg, id:string) {
        obj.beer = new List(this.beers).Where(c => c.id === id).First();
    }

    onEditChangeKeg(id) {
        this.onChangeKeg(this.editing, id);
    }

    onAddChangeKeg(id) {
        this.onChangeKeg(this.adding, id);
    }

    onChangeKeg(obj: IBeerKeg, id:number) {
        obj.keg = new List(this.kegs).Where(c => c.id === id).First();
    }

    formatDate(date) {
        const d = new Date(date);
        let month = `${d.getMonth() + 1}`;
        let day = `${d.getDate()}`;
        const year = d.getFullYear();

        if (month.length < 2) month = `0${month}`;
        if (day.length < 2) day = `0${day}`;

        return [year, month, day].join("-");
    }

    setValue(object, property:string, value) {
        if (object) {
            object[property] = value;
        }
    }
}