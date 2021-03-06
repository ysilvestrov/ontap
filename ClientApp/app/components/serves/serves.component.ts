//import * as ng from '@angular/core';
//import { Http } from '@angular/http';
//import { List } from "../../modules/linq";
//import {IPub, IBeer, IServe, Serve} from "../../models/ontap.models";
//import {EPubService} from "../epubs/epubs.service";
//import {BeerService} from "../beers/beers.service";
//import { ServeService } from "./serves.service";
//import { BjcpStyle, BjcpService } from "../../services/bjcp.service";
//import {AppComponent, AppService, Options} from "../../modules/appComponent";
//import { Locale, LocaleService, LocalizationService } from 'angular2localization';
//import { PrintComponent } from "../print/print.component";

//@ng.Component({
//    selector: 'serves',
//    providers: [ServeService, EPubService, BeerService, BjcpService],
//    templateUrl: './serves.component.html',
//    styleUrls: ['./serves.component.css']
//})
//export class ServesComponent extends  AppComponent<IServe, ServeService> {
//    public serves: IServe[];
//    public pubs: IPub[];
//    public beers: IBeer[];
//    public pub: IPub;
//    public styles: { [code: string]: BjcpStyle };
//    public selectingPubs: Options[];
//    public selectingBeers: Options[];

//    constructor(elmService: ServeService, private pubService: EPubService, private beerService: BeerService,
//        private bjcpService: BjcpService, public locale: LocaleService, public localization: LocalizationService) {
//        super(elmService, locale, localization);
//        this.getPubs();
//        this.getBeers();
//        this.getStyles();
//        if (this.elements) {
//            this.onElementsLoad();
//        }
//        this.onLoad.subscribe((s: any, elements: any) => { this.onElementsLoad() });
//    }

//    public startAdd() {
//        super.startAdd();
//        if (this.pub) {
//            this.adding.servedIn = new List(this.pubs).First(p => p.id === this.pub.id);
//        } else {
//            this.adding.servedIn = new List(this.pubs).First();
//        }
//        this.adding.served = new List(this.beers).First();
//        this.adding.tap = (new List(this.elements).Select(s => s.tap).Max() || 0) + 1;
//        if (this.elements.length > 0) {
//            var last = new List(this.elements).Last();
//            this.adding.volume = last.volume;
//            this.adding.price = last.price;
//        }
//    }

//    getPubs() {
//        this.pubService.get()
//            .subscribe(
//            pubs => {
//                this.pubs = pubs;
//                this.selectingPubs = new List(this.pubs).OrderBy(p => p.name).Select(p => new Options(p.id, p.name))
//                    .ToArray();
//                this.onElementsLoad();
//            },
//            error => this.errorMessage = <any>error);
//    }

//    getStyles() {
//        this.bjcpService.get()
//            .subscribe(
//                styles => {
//                    this.styles = new List(styles).OrderBy((_: BjcpStyle) => _.style).ToDictionary((_: BjcpStyle) => _.code, (_: BjcpStyle) => _.style);
//                },
//                error => this.errorMessage = <any>error);
//    }

//    onElementsLoad() {
//        if (this.selectingPubs.length > 0) {
//            this.setPub(this.selectingPubs[0].label);
//        }
//    }

//    getBeers() {
//        this.beerService.get()
//            .subscribe(
//            beers => {
//                this.beers = beers;
//                this.selectingBeers = new List(this.beers).OrderBy(b => b.brewery.name).ThenBy(b => b.name)
//                    .Select(b => new Options(b.id, b.name + " (" + b.brewery.name + ")"))
//                    .ToArray();
//            },
//            error => this.errorMessage = <any>error);
//    }

//    onEditChangePub(id) {
//        this.onChangePub(this.editing, id);
//    }

//    onAddChangePub(id) {
//        this.onChangePub(this.adding, id);
//    }

//    onChangePub(obj: IServe, id:string) {
//        obj.servedIn = new List(this.pubs).Where(c => c.id === id).First();
//    }

//    onEditChangeBeer(id) {
//        this.onChangeBeer(this.editing, id);
//    }

//    onAddChangeBeer(id) {
//        this.onChangeBeer(this.adding, id);
//    }

//    onChangeBeer(obj: IServe, id:string) {
//        obj.served = new List(this.beers).Where(c => c.id === id).First();
//    }

//    public setPub(name) {
//        if (!this.serves) {
//            this.serves = this.elements;
//        }
//        this.pub = new List(this.pubs)
//            .Where((pub) => pub.name === name)
//            .First();
//        this.elements = (name === "") ? this.serves :
//            new List(this.serves)
//                .Where((serve) => serve.servedIn.id === this.pub.id).OrderBy((serve) => serve.tap)
//                .ToArray();
//    }
//}