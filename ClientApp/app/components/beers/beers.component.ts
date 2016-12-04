import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { FormsModule }   from '@angular/forms';
import { List } from "../../modules/linq";
import {IBeer, Beer, IBrewery} from "../../models/ontap.models";
import {BeerService} from "./beers.service";
import {BreweryService} from "../breweries/breweries.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";
import { CloudinaryOptions, CloudinaryUploader } from 'ng2-cloudinary';
import { FileSelectDirective, FileDropDirective, FileUploader } from 'ng2-file-upload';
import { Ng2BootstrapModule, AlertModule } from "ng2-bootstrap/ng2-bootstrap";

@ng.Component({
    selector: 'beers',
    providers: [BeerService, BreweryService],
    styles: [require('./beers.component.css')],
  template: require('./beers.component.html')
})
export class BeersComponent extends AppComponent<IBeer, BeerService> {

    public breweries: IBrewery[];
    public allBeers: IBeer[];
    public brewery: IBrewery;
    cloudinaryImage: any;

    cloudinaryOptions: CloudinaryOptions = new CloudinaryOptions({
        cloud_name: 'ontap-in-ua',
        upload_preset: 'ontapInUa_pubs',
        autoUpload: true
    });

    uploader: CloudinaryUploader = new CloudinaryUploader(this.cloudinaryOptions);

    constructor(elmService: BeerService, private breweryService: BreweryService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getBreweries();
        if (this.elements) {
            this.onElementsLoad(this.elements);
        }
        this.onLoad.subscribe((s: BeersComponent, elements: IBeer[]) => { this.onElementsLoad(elements) });

        //Override onSuccessItem function to record cloudinary response data
        this.uploader.onSuccessItem = (item: any, response: string, status: number, headers: any) => {
            //response is the cloudinary response
            //see http://cloudinary.com/documentation/upload_images#upload_response
            this.cloudinaryImage = JSON.parse(response);
            if (this.editing)
                this.editing.image = this.cloudinaryImage.public_id;
            if (this.adding)
                this.adding.image = this.cloudinaryImage.public_id;

            return { item, response, status, headers };
        };
    }

    onElementsLoad(elements: IBeer[]) {
        this.allBeers = elements;
        this.brewery = null;
        this.setBrewery("");
    }

    public setBrewery(name) {
        this.brewery = new List(this.breweries)
            .Where((brewery) => brewery.name === name)
            .First();
        this.elements = (name === "") ?
            new List(this.allBeers)
                .OrderBy((beer: IBeer) => beer.brewery.name)
                .ToArray() :
            new List(this.allBeers)
                .Where((beer) => beer.brewery.name === this.brewery.name)
                .OrderBy((beer: IBeer) => beer.brewery.name)
                .ToArray();
    }

    startAdd() {
        super.startAdd();
        this.adding.brewery = new List(this.breweries).First();
    }

    getBreweries() {
        this.breweryService.get()
            .subscribe(
            breweries => {
                this.breweries = new List(breweries).OrderBy((_: IBrewery) => _.name).ToArray();
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