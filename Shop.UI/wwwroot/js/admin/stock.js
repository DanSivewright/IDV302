var app = new Vue({
    el: '#app',
    data: {
        loading: false,
        products: [],
        selectedProduct: null,
        newStock: {
            productId: 0,
            description: "Size",
            quantity: 10
        },
    },
    mounted() {
        this.getStock();
    },
    methods: {
        getStock() {
            this.loading = true
            axios.get('/Admin/stocks')
                .then(res => {
                    this.products = res.data;
                    console.log(this.products)
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        updateStock() {
            this.loading = true
            axios
                .put('/Admin/stocks', {
                    stock: this.selectedProduct.stock.map(x => {
                        return {
                            id: x.id,
                            description: x.description,
                            quantity: x.quantity,
                            productId: this.selectedProduct.id
                        };
                    })
                })
                .then(res => {
                    console.log(res.data)
                    this.selectedProduct.stock.splice(index, 1)
                })
                .catch(err => {
                    console.log(err)
                })          
        },
        deleteStock: function(id, index) {
            this.loading = true
            axios.delete('/Admin/stocks/' + id)
                .then(res => {
                    this.selectedProduct.stock.splice(index, 1)
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },  
        addStock() {
            this.loading = true
            axios.post('/Admin/stocks', this.newStock)
                .then(res => {
                    this.selectedProduct.stock.push(res.data);
                    console.log(res.data)
                    this.loading = false
                })
                .catch(err => {
                    console.log(err.response)
                    this.loading = false
                })
        },
        selectProduct(product) {
            this.selectedProduct = product
            this.newStock.productId = product.id; 
        },
    }
})