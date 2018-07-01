import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { TimerObservable } from "rxjs/observable/TimerObservable";
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, ICity, Pub} from "../../models/ontap.models";
import {PubService} from "../pubs/pubs.service";
import {CityService} from "../cities/cities.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import {SortByTap} from "../app/sortbytap.pipe";
import { TooltipContainerComponent, TooltipDirective, TooltipModule, Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';
import * as moment from 'moment';
import { LocaleService, LocalizationService } from 'angular2localization';
import { CloudinaryOptions } from 'ng2-cloudinary';
import { Router, ActivatedRoute } from '@angular/router';
import Ontapmodels = require("../../models/ontap.models");
import IPubServe = Ontapmodels.IPubServe;
import Epubsservice = require("../epubs/epubs.service");
import EPubService = Epubsservice.EPubService;

@Component({
    selector: 'pubs',
    providers: [PubService],
    styles: [require('./pubs.component.css')],
  template: require('./pubs.component.html')
})
export class PubsComponent extends AppComponent<IPubServe, PubService> implements OnInit, OnDestroy {
    public allPubs: IPubServe[];
    public cities: ICity[];
    public city: ICity;
    public pubUpdates: {};

    public isBrowser: boolean;
    public isLoaded: boolean;
    public subscription: Subscription;

    cloudinaryOptions: CloudinaryOptions = new CloudinaryOptions({
        cloudName: 'ontap-in-ua',
        uploadPreset: 'ontapInUa_pubs',
        autoUpload: true
    });

    constructor(elmService: PubService, public locale: LocaleService, public localization: LocalizationService,
        private route: ActivatedRoute,
        private router: Router) {
        super(elmService, locale, localization);

        try {
            if (sessionStorage) {
                let is18 = sessionStorage.getItem("is18");
                if (!is18 || is18 !== "true") {
                    sessionStorage.setItem("goTo", JSON.stringify(['/pubs']));
                    this.router.navigate(['/home']);
                }
            }
        } catch (e) {
            //do nothing
        } 

        if (this.elements) {
            this.onElementsLoad(this.elements);
        }
        this.onLoad.subscribe((s: PubsComponent, elements: IPubServe[]) => { this.onElementsLoad(elements) });
        moment.locale(this.locale.getCurrentLanguage());

        this.isBrowser = typeof (document) != "undefined";
        this.isLoaded = false;
        this.localization.translationChanged.subscribe(

            // Refreshes the variable 'title' with the new translation when the selected language changes.
            () => {
                this.isLoaded = true;
                moment.locale(this.locale.getCurrentLanguage());
                this.recalcDates();
            }

        );
    }

    ngOnInit() {
        if (this.isBrowser) {
            let timer = TimerObservable.create(10 * 1000, 15 * 60 * 1000);
            this.subscription = timer.subscribe(t => {
                this.get();
            });
        }
    }

    ngOnDestroy() {
        if (typeof (this.subscription) != "undefined") {
            this.subscription.unsubscribe();
        }
    }

    onElementsLoad(elements:IPubServe[]) {
        this.allPubs = elements;
        this.city = null;
        var pubCities = new List(this.allPubs)
            .Select((pub: IPub) => pub.city).ToArray();
        this.cities = new List(pubCities)
            .DistinctBy((city: ICity) => city.name)
            .OrderBy((city: ICity) => city.name)
            .ToArray();
        this.setCity("");
        this.recalcDates();     
    }

   public recalcDates() {
       var allDates = new List(this.allPubs).Select(p => [p.id, p.lastUpdated]);
       var selectedDates = allDates.Where(t => typeof (t[1]) != "undefined" && t[1] && moment(t[1]).diff(moment().subtract(1, "year")) > 0);
       this.pubUpdates = selectedDates.ToDictionary(t => t[0], t => moment.utc(t[1]).local().calendar());          
   }

    public setCity(name) {
        this.city = new List(this.cities)
            .Where((city) => city.name === name)
            .First();
        this.elements = (name === "") ? 
            new List(this.allPubs)
                .OrderByDescending((pub: IPubServe) => pub.serves.length)
                .ToArray() :
            new List(this.allPubs)
                .Where((pub) => pub.city.name === this.city.name)
                .OrderByDescending((pub: IPubServe) => pub.serves.length)
                .ToArray();
    }
}
