import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import { ITap, IBeerKeg, IBeerKegOnTap } from "../../models/ontap.models";
import {TapService} from "../taps/taps.service";
import {BeerKegService} from "../beerkegs/beerkegs.service";
import { BeerKegsOnTapService } from "./beerkegsontap.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'beerkegsontap',
    providers: [BeerKegsOnTapService, TapService, BeerKegService],
  template: require('./beerkegsontap.component.html')
})
export class BeerKegsOnTapComponent extends AppComponent<IBeerKegOnTap, BeerKegsOnTapService> {
    public taps: ITap[];
    public kegs: IBeerKeg[];

    constructor(elmService: BeerKegsOnTapService,
        private tapService: TapService,
        private kegService: BeerKegService,
        public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getTaps();
        this.getKegs();
    }

    public startAdd() {
        super.startAdd();
        this.adding.tap = new List(this.taps).First();
        this.adding.keg = new List(this.kegs).First();
    } 

    getTaps() {
        this.tapService.get()
            .subscribe(
            taps => this.taps = taps,
            error => this.errorMessage = <any>error);
    }

    getKegs() {
        this.kegService.get()
            .subscribe(
            kegs => this.kegs = kegs,
            error => this.errorMessage = <any>error);
    }

    onEditChangeTap(id) {
        this.onChangeTap(this.editing, id);
    }

    onAddChangeTap(id) {
        this.onChangeTap(this.adding, id);
    }

    onChangeTap(obj: IBeerKegOnTap, id:number) {
        obj.tap = new List(this.taps).Where(c => c.id === id).First();
    }

    onEditChangeKeg(id) {
        this.onChangeKeg(this.editing, id);
    }

    onAddChangeKeg(id) {
        this.onChangeKeg(this.adding, id);
    }

    onChangeKeg(obj: IBeerKegOnTap, id:number) {
        obj.keg = new List(this.kegs).Where(c => c.id === id).First();
    }
}