import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { FormsModule }   from '@angular/forms';
import { List } from "../../modules/linq";
import {IBeer, Beer, IBrewery} from "../../models/ontap.models";
import {BeerService} from "./beers.service";
import { BreweryService } from "../breweries/breweries.service";
import { BjcpStyle, BjcpService} from "../../services/bjcp.service";
import { AppComponent, AppService, Options, LabeledCloudinaryUploader} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";
import { CloudinaryOptions, CloudinaryUploader } from 'ng2-cloudinary';
import { FileSelectDirective, FileDropDirective, FileUploader } from 'ng2-file-upload';
import { Ng2BootstrapModule, AlertModule } from "ng2-bootstrap/ng2-bootstrap";

@ng.Component({
    selector: 'beers',
    providers: [BeerService, BreweryService, BjcpService],
    templateUrl: './beers.component.html',
    styleUrls: ['./beers.component.css']

})
export class BeersComponent extends AppComponent<IBeer, BeerService> {

    public breweries: IBrewery[];
    public allBeers: IBeer[];
    public brewery: IBrewery;
    public selectingBreweries: Options[];
    public styles: {[code:string]:BjcpStyle};
    public selectingStyles: Options[];

    constructor(elmService: BeerService, private breweryService: BreweryService, private bjcpService: BjcpService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization, [new LabeledCloudinaryUploader(new CloudinaryOptions({
            cloudName: 'ontap-in-ua',
            uploadPreset: 'ontapInUa_pubs'
        }))]);
        this.getBreweries();
        this.getStyles();
        if (this.elements) {
            this.onElementsLoad(this.elements);
        }
        this.onLoad.subscribe((s: BeersComponent, elements: IBeer[]) => { this.onElementsLoad(elements) });
    }

    onElementsLoad(elements: IBeer[]) {
        this.allBeers = elements;
        this.brewery = null;
        this.setBrewery("");
    }

    public setBrewery(id) {
        this.brewery = new List(this.breweries)
            .Where((brewery) => brewery.id === id)
            .First();
        this.elements = (id === "") ?
            new List(this.allBeers)
                .OrderBy((beer: IBeer) => beer.brewery.name + " " + beer.name)
                .ToArray() :
            new List(this.allBeers)
                .Where((beer) => beer.brewery.name === this.brewery.name)
                .OrderBy((beer: IBeer) => beer.name)
                .ToArray();
    }

    startAdd() {
        super.startAdd();
        this.adding.brewery = this.brewery || new List(this.breweries).First();
        this.adding.name = '';
        this.adding.description = '';
        this.adding.alcohol = 5;
        this.adding.gravity = 0;
        this.adding.ibu = 0;
    }

    getBreweries() {
        this.breweryService.get()
            .subscribe(
            breweries => {
                this.breweries = new List(breweries).OrderBy((_: IBrewery) => _.name).ToArray();
                this.selectingBreweries = new List(this.breweries).OrderBy(b => b.name).Select(b => new Options(b.id, b.name))
                    .ToArray();
            },
            error => this.errorMessage = <any>error);
    }

    getStyles() {
        this.bjcpService.get()
            .subscribe(
            styles => {
                this.styles = new List(styles).OrderBy((_: BjcpStyle) => _.style).ToDictionary((_: BjcpStyle) => _.code, (_: BjcpStyle) => _.style);
                this.selectingStyles = new List(styles).OrderBy((_: BjcpStyle) => _.style).Select(s => new Options(s.code, s.style)).ToArray();
            },
            error => this.errorMessage = <any>error);
    }

    onEditChangeBrewery(id) {
        this.onChangeBrewery(this.editing, id);
    }

    onAddChangeBrewery(id) {
        this.onChangeBrewery(this.adding, id);
    }

    onChangeBrewery(obj: IBeer, id: string) {
        obj.brewery = new List(this.breweries).Where(c => c.id === id).First();
    }

}