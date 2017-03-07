import * as ng from "@angular/core";
import { Http } from "@angular/http";
import { List } from "../../modules/linq";
import {IBrewery, Brewery, ICountry, IBeer} from "../../models/ontap.models";
import {BreweryService} from "./breweries.service";
import {CountryService} from "../countries/countries.service";
import {BeerService} from "../beers/beers.service";
import {SelectorComponent} from "../selector/selector.component";
import {AppComponent, AppService, Options} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";
import { CloudinaryOptions, CloudinaryUploader } from "ng2-cloudinary";
import { FileSelectDirective, FileDropDirective, FileUploader } from "ng2-file-upload";
import { DialogService } from "ng2-bootstrap-modal";

@ng.Component({
    selector: "breweries",
    providers: [BreweryService, CountryService, BeerService],
    styles: [require("./breweries.component.css")],
    template: require("./breweries.component.html")
})
export class BreweriesComponent extends AppComponent<IBrewery, BreweryService> {

    public countries: ICountry[];
    public beers: IBeer[];
    public breweryCounts = {};

    constructor(elmService: BreweryService,
        private dialogService: DialogService,
        private countryService: CountryService,
        private beerService: BeerService,
        public locale: LocaleService,
        public localization: LocalizationService)
    {
        super(elmService, locale, localization, new CloudinaryUploader(new CloudinaryOptions({
            cloudName: "ontap-in-ua",
            uploadPreset: "ontapInUa_breweries"
        })));
        this.getCountries();
        this.getBeers();
        this.onDelete.subscribe((s: BreweriesComponent, res: [IBrewery, any]) => { this.onElementDeleted(res[0], res[1]) });
    }

    startAdd() {
        super.startAdd();
        this.adding.country = new List(this.countries).First();
    }

    getCountries() {
        this.countryService.get()
            .subscribe(
            countries => this.countries = countries,
            error => this.errorMessage = error);
    }

    getBeers() {
        this.beerService.get()
            .subscribe(
            beers => {
                this.beers = beers;
                this.breweryCounts = new List(beers)
                    .Aggregate((ac, b: IBeer) => {
                        ac[b.brewery.id]
                            ? ac[b.brewery.id] += 1
                            : ac[b.brewery.id] = 1;
                        return ac;
                    }, {});
                this.get();
            },
            error => this.errorMessage = error);
    }

    onEditChangeCountry(id) {
        this.onChangeCountry(this.editing, id);
    }

    onAddChangeCountry(id) {
        this.onChangeCountry(this.adding, id);
    }

    onChangeCountry(obj: IBrewery, id: string) {
        obj.country = new List(this.countries).Where(c => c.id === id).First();
    }
    startDelete() {
        if (!this.editing)
            return;
        if (this.breweryCounts[this.editing.id]) {
            let disposable = this.dialogService.addDialog(SelectorComponent, {
                title: 'Заменить на',
                message: 'У выбранной пивоварни есть ' + this.breweryCounts[this.editing.id] +' сортов пива. Выберите замену:',
                options: new List(this.elements).Where(b => b.id !== this.editing.id).OrderBy(b => b.name).Select(b => new Options(b.id, b.name)).ToArray()
            })
                .subscribe((replacement) => {
                    //We get dialog result
                    if (replacement) {
                        super.delete(replacement);
                    }
                    else {
                        return;
                    }
                });
            //We can close dialog calling disposable.unsubscribe();
            //If dialog was not closed manually close it by timeout
            //setTimeout(() => {
            //    disposable.unsubscribe();
            //}, 10000);
        } else {
            super.delete();
        }
    }

    onElementDeleted(deleted: IBrewery, replacement: any) {
        if (replacement) {
            this.breweryCounts[replacement] = (this.breweryCounts[replacement] || 0) + this.breweryCounts[deleted.id];
        }                
    }
}