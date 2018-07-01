import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { TimerObservable } from "rxjs/observable/TimerObservable";
import 'rxjs/add/operator/switchMap';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, ICity, Pub} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {CityService} from "../cities/cities.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import {SortByTap} from "../app/sortbytap.pipe";
import { TooltipContainerComponent, TooltipDirective, TooltipModule, Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';
import {PrintComponent} from "../print/print.component";
import * as moment from 'moment';
import { Locale, LocaleService, LocalizationService } from 'angular2localization';
import { CloudinaryOptions } from 'ng2-cloudinary';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    selector: 'pub',
    providers: [EPubService],
    templateUrl: './pub.component.html',
    styleUrls: ['./pub.component.css']
})
export class PubComponent extends Locale implements OnInit {
    public pub;

    public isBrowser: boolean;
    public isLoaded: boolean;
    public subscription: Subscription;

    cloudinaryOptions: CloudinaryOptions = new CloudinaryOptions({
        cloudName: 'ontap-in-ua',
        uploadPreset: 'ontapInUa_pubs',
        autoUpload: true
    });

    constructor(
        private elmService: EPubService,
        public locale: LocaleService,
        public localization: LocalizationService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(locale, localization);

        try {
            if (sessionStorage) {
                let is18 = sessionStorage.getItem("is18");
                if (!is18 || is18 !== "true") {
                    this.route.params.subscribe(params => {
                        let id = params['id'];
                        sessionStorage.setItem("goTo", JSON.stringify(['/pub', id]));
                        this.router.navigate(['/home']);
                    });
                }
            }
        } catch (e) {
            //do nothing
        } 

        moment.locale(this.locale.getCurrentLanguage());
        this.isBrowser = typeof (document) != "undefined";
        this.isLoaded = false;
        this.localization.translationChanged.subscribe(

            // Refreshes the variable 'title' with the new translation when the selected language changes.
            () => {
                this.isLoaded = true;
                moment.locale(this.locale.getCurrentLanguage());
            }

        );
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            let id = params['id'];
            this.elmService.getOne(id).subscribe((pub: Pub) => this.pub = pub);
        });
    }

}
