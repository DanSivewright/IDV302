var app = new Vue({
    el: '#app',
    data: {
        editing: false,
        loading: false,
        objectIndex: 0,
        productModel: {
            id: 0,
            name: "Product Name",
            description: "Product Description",
            value: 1.99
        },
        products: []
    },
    mounted() {
        this.getProducts();
    },
    methods: {
        getProduct(id) {
            this.loading = true
            axios
                .get('/Admin/products' + id)
                .then(res => {
                    this.loading = false
                    var product = res.data;
                    this.productModel = {
                        id: product.id,
                        name: product.name,
                        description: product.description,
                        value: product.value
                    }

                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        getProducts() {
            this.loading = true
            axios.get('/Admin/products')
                .then(res => {
                    this.products = res.data;
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        createProduct() {
            this.loading = true

            axios
                .post('/Admin/products', this.productModel)
                .then(res => {
                    this.products.push(res.data)
                    this.loading = false
                    this.editing = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        updateProduct() {
            this.loading = true
            axios
                .put('Admin/products', this.productModel)
                .then(res => {
                    
                    this.products.splice(this.objectIndex, 1, res.data)
                    this.editing = false
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        deleteProduct(id, index) {
            this.loading = true
            axios
                .delete('/Admin/products/' + id)
                .then(res => {
                    this.products.splice(index, 1)
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        newProduct() {
            this.editing = true
            this.productModel.id = 0
        },
        editProduct(id, index) {
            this.objectIndex = index
            this.getProduct(id)
            this.editing = true
        },
        cancel() {
            this.editing = false
        }
    },
    computed: {

    }
})