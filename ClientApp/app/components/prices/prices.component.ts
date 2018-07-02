import * as ng from "@angular/core";
import { Http } from "@angular/http";
import { List } from "../../modules/linq";
import {IPub, IBeer, IBeerPrice} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {BeerService} from "../beers/beers.service";
import {BeerPriceService} from "./prices.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: "prices",
    providers: [BeerPriceService, EPubService, BeerService],
  template: require("./prices.component.html")
})
export class PricesComponent extends  AppComponent<IBeerPrice, BeerPriceService> {
    public pubs: IPub[];
    public beers: IBeer[];

    constructor(elmService: BeerPriceService, private pubService: EPubService, private beerService: BeerService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getPubs();
        this.getBeers();
    }

    public startAdd() {
        super.startAdd();
        this.adding.pub = new List(this.pubs).First();
        this.adding.beer = new List(this.beers).First();
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

    onChangePub(obj: IBeerPrice, id:string) {
        obj.pub = new List(this.pubs).Where(c => c.id === id).First();
    }

    onEditChangeBeer(id) {
        this.onChangeBeer(this.editing, id);
    }

    onAddChangeBeer(id) {
        this.onChangeBeer(this.adding, id);
    }

    onChangeBeer(obj: IBeerPrice, id:string) {
        obj.beer = new List(this.beers).Where(b => b.id === id).First();
    }
}