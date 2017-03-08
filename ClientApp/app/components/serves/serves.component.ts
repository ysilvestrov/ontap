import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, IBeer, IServe, Serve} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {BeerService} from "../beers/beers.service";
import {ServeService} from "./serves.service";
import {AppComponent, AppService, Options} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from 'angular2localization';

@ng.Component({
    selector: 'serves',
    providers: [ServeService, EPubService, BeerService],
  template: require('./serves.component.html')
})
export class ServesComponent extends  AppComponent<IServe, ServeService> {
    public serves: IServe[];
    public pubs: IPub[];
    public beers: IBeer[];
    public pub: IPub;
    public selectingPubs: Options[];
    public selectingBeers: Options[];

    constructor(elmService: ServeService, private pubService: EPubService, private beerService: BeerService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getPubs();
        this.getBeers();
    }

    public startAdd() {
        super.startAdd();
        if (this.pub) {
            this.adding.servedIn = new List(this.pubs).First(p => p.id === this.pub.id);
        } else {
            this.adding.servedIn = new List(this.pubs).First();
        }
        this.adding.served = new List(this.beers).First();
    }

    getPubs() {
        this.pubService.get()
            .subscribe(
            pubs => {
                this.pubs = pubs;
                this.selectingPubs = new List(this.pubs).OrderBy(p => p.name).Select(p => new Options(p.id, p.name))
                    .ToArray();
            },
            error => this.errorMessage = <any>error);
    }

    getBeers() {
        this.beerService.get()
            .subscribe(
            beers => {
                this.beers = beers;
                this.selectingBeers = new List(this.beers).OrderBy(b => b.brewery.name).ThenBy(b => b.name)
                    .Select(b => new Options(b.id, b.name + " (" + b.brewery.name + ")"))
                    .ToArray();
            },
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

    public setPub(name) {
        if (!this.serves) {
            this.serves = this.elements;
        }
        this.pub = new List(this.pubs)
            .Where((pub) => pub.name === name)
            .First();
        this.elements = (name === "") ? this.serves :
            new List(this.serves)
                .Where((serve) => serve.servedIn.name === this.pub.name).OrderBy((serve) => serve.tap)
                .ToArray();
    }
}