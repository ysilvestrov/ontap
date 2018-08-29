import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import { IBeerKegWeight, IBeerKeg, IBeerKegOnTap } from "../../models/ontap.models";
import {TapService} from "../taps/taps.service";
import {BeerKegService} from "../beerkegs/beerkegs.service";
import { BeerKegWeightsService } from "./beerkegweights.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'beerkegweights',
    providers: [BeerKegWeightsService, BeerKegService],
  template: require('./beerkegweights.component.html')
})
export class BeerKegWeightsComponent extends AppComponent<IBeerKegWeight, BeerKegWeightsService> {
    public kegs: IBeerKeg[];

    constructor(elmService: BeerKegWeightsService,
        private kegService: BeerKegService,
        public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getKegs();
    }

    public startAdd() {
        super.startAdd();
        this.adding.keg = new List(this.kegs).First();
    } 

    getKegs() {
        this.kegService.get()
            .subscribe(
            kegs => this.kegs = kegs,
            error => this.errorMessage = <any>error);
    }

    onEditChangeKeg(id) {
        this.onChangeKeg(this.editing, id);
    }

    onAddChangeKeg(id) {
        this.onChangeKeg(this.adding, id);
    }

    onChangeKeg(obj: IBeerKegWeight, id:number) {
        obj.keg = new List(this.kegs).Where(c => c.id === id).First();
    }
}