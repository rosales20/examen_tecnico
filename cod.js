        // Simulación de base de datos en memoria
        let productos = [];
        let compras = [];
        let ventas = [];
        let movimientos = [];
        let nextId = 1;

        // Funciones de navegación
        function showSection(section) {
            document.querySelectorAll('.section').forEach(s => s.style.display = 'none');
            document.getElementById(section + '-section').style.display = 'block';
            
            if (section === 'dashboard') {
                actualizarDashboard();
            } else if (section === 'productos') {
                cargarProductos();
            } else if (section === 'compras') {
                cargarCompras();
                cargarProductosSelect('compra-producto');
            } else if (section === 'ventas') {
                cargarVentas();
                cargarProductosSelect('venta-producto');
            }
        }

        // Funciones de productos
        function guardarProducto() {
            const nombre = document.getElementById('producto-nombre').value;
            const stock = parseInt(document.getElementById('producto-stock').value);
            const costo = parseFloat(document.getElementById('producto-costo').value);
            const precio = parseFloat(document.getElementById('producto-precio').value);

            const producto = {
                id: nextId++,
                nombre,
                stock,
                costo,
                precio
            };

            productos.push(producto);
            
            // Registrar movimiento inicial
            movimientos.push({
                id: nextId++,
                productoId: producto.id,
                tipo: 'Entrada',
                cantidad: stock,
                stockAnterior: 0,
                stockActual: stock,
                costo: costo,
                precioVenta: precio,
                fecha: new Date().toISOString().split('T')[0]
            });

            document.getElementById('producto-form').reset();
            bootstrap.Modal.getInstance(document.getElementById('productoModal')).hide();
            cargarProductos();
            mostrarMensaje('Producto guardado exitosamente', 'success');
        }

        function cargarProductos() {
            const tbody = document.getElementById('productos-table');
            tbody.innerHTML = '';

            productos.forEach(producto => {
                const stockClass = producto.stock < 10 ? 'text-danger' : '';
                tbody.innerHTML += `
                    <tr>
                        <td>${producto.id}</td>
                        <td>${producto.nombre}</td>
                        <td class="${stockClass}">${producto.stock}</td>
                        <td>S/ ${producto.costo.toFixed(2)}</td>
                        <td>S/ ${producto.precio.toFixed(2)}</td>
                        <td>
                            <button class="btn btn-sm btn-info" onclick="verKardex(${producto.id})">
                                <i class="fas fa-list"></i> Kardex
                            </button>
                            <button class="btn btn-sm btn-danger" onclick="eliminarProducto(${producto.id})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
        }

        function eliminarProducto(id) {
            if (confirm('¿Está seguro de eliminar este producto?')) {
                productos = productos.filter(p => p.id !== id);
                cargarProductos();
                mostrarMensaje('Producto eliminado exitosamente', 'success');
            }
        }

        // Funciones de compras
        function cargarProductosSelect(selectId) {
            const select = document.getElementById(selectId);
            select.innerHTML = '<option value="">Seleccione un producto</option>';
            
            productos.forEach(producto => {
                select.innerHTML += `<option value="${producto.id}">${producto.nombre}</option>`;
            });
        }

        function guardarCompra() {
            const productoId = parseInt(document.getElementById('compra-producto').value);
            const cantidad = parseInt(document.getElementById('compra-cantidad').value);
            const precio = parseFloat(document.getElementById('compra-precio').value);
            const total = cantidad * precio;

            const compra = {
                id: nextId++,
                productoId,
                cantidad,
                precio,
                total,
                fecha: new Date().toISOString().split('T')[0]
            };

            compras.push(compra);

            // Actualizar stock del producto
            const producto = productos.find(p => p.id === productoId);
            const stockAnterior = producto.stock;
            producto.stock += cantidad;

            // Registrar movimiento
            movimientos.push({
                id: nextId++,
                productoId: productoId,
                tipo: 'Entrada',
                cantidad: cantidad,
                stockAnterior: stockAnterior,
                stockActual: producto.stock,
                costo: precio,
                precioVenta: producto.precio,
                fecha: compra.fecha
            });

            document.getElementById('compra-form').reset();
            bootstrap.Modal.getInstance(document.getElementById('compraModal')).hide();
            cargarCompras();
            mostrarMensaje('Compra registrada exitosamente', 'success');
        }

        function cargarCompras() {
            const tbody = document.getElementById('compras-table');
            tbody.innerHTML = '';

            compras.forEach(compra => {
                const producto = productos.find(p => p.id === compra.productoId);
                tbody.innerHTML += `
                    <tr>
                        <td>${compra.id}</td>
                        <td>${producto ? producto.nombre : 'Producto eliminado'}</td>
                        <td>${compra.cantidad}</td>
                        <td>S/ ${compra.precio.toFixed(2)}</td>
                        <td>S/ ${compra.total.toFixed(2)}</td>
                        <td>${compra.fecha}</td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="eliminarCompra(${compra.id})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
        }

        function eliminarCompra(id) {
            if (confirm('¿Está seguro de eliminar esta compra?')) {
                compras = compras.filter(c => c.id !== id);
                cargarCompras();
                mostrarMensaje('Compra eliminada exitosamente', 'success');
            }
        }

        // Funciones de ventas
        function guardarVenta() {
            const productoId = parseInt(document.getElementById('venta-producto').value);
            const cantidad = parseInt(document.getElementById('venta-cantidad').value);
            const precio = parseFloat(document.getElementById('venta-precio').value);
            
            const producto = productos.find(p => p.id === productoId);
            
            if (producto.stock < cantidad) {
                mostrarMensaje('Stock insuficiente para realizar la venta', 'danger');
                return;
            }

            const subtotal = cantidad * precio;
            const igv = subtotal * 0.18;
            const total = subtotal + igv;

            const venta = {
                id: nextId++,
                productoId,
                cantidad,
                precio,
                subtotal,
                igv,
                total,
                fecha: new Date().toISOString().split('T')[0]
            };

            ventas.push(venta);

            // Actualizar stock del producto
            const stockAnterior = producto.stock;
            producto.stock -= cantidad;

            // Registrar movimiento
            movimientos.push({
                id: nextId++,
                productoId: productoId,
                tipo: 'Salida',
                cantidad: cantidad,
                stockAnterior: stockAnterior,
                stockActual: producto.stock,
                costo: producto.costo,
                precioVenta: precio,
                fecha: venta.fecha
            });

            document.getElementById('venta-form').reset();
            bootstrap.Modal.getInstance(document.getElementById('ventaModal')).hide();
            cargarVentas();
            mostrarMensaje('Venta registrada exitosamente', 'success');
        }

        function cargarVentas() {
            const tbody = document.getElementById('ventas-table');
            tbody.innerHTML = '';

            ventas.forEach(venta => {
                const producto = productos.find(p => p.id === venta.productoId);
                tbody.innerHTML += `
                    <tr>
                        <td>${venta.id}</td>
                        <td>${producto ? producto.nombre : 'Producto eliminado'}</td>
                        <td>${venta.cantidad}</td>
                        <td>S/ ${venta.precio.toFixed(2)}</td>
                        <td>S/ ${venta.subtotal.toFixed(2)}</td>
                        <td>S/ ${venta.igv.toFixed(2)}</td>
                        <td>S/ ${venta.total.toFixed(2)}</td>
                        <td>${venta.fecha}</td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="eliminarVenta(${venta.id})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
        }

        function eliminarVenta(id) {
            if (confirm('¿Está seguro de eliminar esta venta?')) {
                ventas = ventas.filter(v => v.id !== id);
                cargarVentas();
                mostrarMensaje('Venta eliminada exitosamente', 'success');
            }
        }

        // Función para ver Kardex
        function verKardex(productoId) {
            const producto = productos.find(p => p.id === productoId);
            const movimientosProducto = movimientos.filter(m => m.productoId === productoId);
            
            document.getElementById('kardex-producto-nombre').textContent = `Kardex: ${producto.nombre}`;
            
            const tbody = document.getElementById('kardex-table');
            tbody.innerHTML = '';
            
            movimientosProducto.forEach(mov => {
                const tipoClass = mov.tipo === 'Entrada' ? 'text-success' : 'text-danger';
                tbody.innerHTML += `
                    <tr>
                        <td>${mov.fecha}</td>
                        <td class="${tipoClass}">${mov.tipo}</td>
                        <td>${mov.cantidad}</td>
                        <td>${mov.stockActual}</td>
                        <td>S/ ${mov.costo.toFixed(2)}</td>
                        <td>S/ ${mov.precioVenta.toFixed(2)}</td>
                    </tr>
                `;
            });
            
            new bootstrap.Modal(document.getElementById('kardexModal')).show();
        }

        // Función para actualizar dashboard
        function actualizarDashboard() {
            document.getElementById('total-productos').textContent = productos.length;
            document.getElementById('total-compras').textContent = compras.length;
            document.getElementById('total-ventas').textContent = ventas.length;
            
            const productosStockBajo = productos.filter(p => p.stock < 10).length;
            document.getElementById('productos-stock-bajo').textContent = productosStockBajo;
        }

        // Función para mostrar mensajes
        function mostrarMensaje(mensaje, tipo) {
            const alertDiv = document.createElement('div');
            alertDiv.className = `alert alert-${tipo} alert-dismissible fade show position-fixed`;
            alertDiv.style.top = '20px';
            alertDiv.style.right = '20px';
            alertDiv.style.zIndex = '9999';
            alertDiv.innerHTML = `
                ${mensaje}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            document.body.appendChild(alertDiv);
            
            setTimeout(() => {
                alertDiv.remove();
            }, 3000);
        }

        // Event listeners para cálculos automáticos
        document.getElementById('compra-cantidad').addEventListener('input', calcularTotalCompra);
        document.getElementById('compra-precio').addEventListener('input', calcularTotalCompra);

        function calcularTotalCompra() {
            const cantidad = parseFloat(document.getElementById('compra-cantidad').value) || 0;
            const precio = parseFloat(document.getElementById('compra-precio').value) || 0;
            document.getElementById('compra-total').value = (cantidad * precio).toFixed(2);
        }

        // Event listeners para ventas
        document.getElementById('venta-producto').addEventListener('change', function() {
            const productoId = parseInt(this.value);
            if (productoId) {
                const producto = productos.find(p => p.id === productoId);
                document.getElementById('stock-disponible').textContent = producto.stock;
                document.getElementById('venta-precio').value = producto.precio.toFixed(2);
                document.getElementById('venta-cantidad').max = producto.stock;
                calcularTotalVenta();
            } else {
                document.getElementById('stock-disponible').textContent = '0';
                document.getElementById('venta-precio').value = '';
                document.getElementById('venta-cantidad').max = '';
            }
        });

        document.getElementById('venta-cantidad').addEventListener('input', function() {
            const productoId = parseInt(document.getElementById('venta-producto').value);
            const cantidad = parseInt(this.value) || 0;
            
            if (productoId) {
                const producto = productos.find(p => p.id === productoId);
                if (cantidad > producto.stock) {
                    this.value = producto.stock;
                    mostrarMensaje('La cantidad no puede ser mayor al stock disponible', 'warning');
                }
            }
            calcularTotalVenta();
        });

        document.getElementById('venta-precio').addEventListener('input', calcularTotalVenta);

        function calcularTotalVenta() {
            const cantidad = parseFloat(document.getElementById('venta-cantidad').value) || 0;
            const precio = parseFloat(document.getElementById('venta-precio').value) || 0;
            const subtotal = cantidad * precio;
            const igv = subtotal * 0.18;
            const total = subtotal + igv;

            document.getElementById('venta-subtotal').value = subtotal.toFixed(2);
            document.getElementById('venta-igv').value = igv.toFixed(2);
            document.getElementById('venta-total').value = total.toFixed(2);
        }

        // API para Backend




        // 🔧 CONFIGURACIÓN DE CONEXIÓN CON EL BACKEND
const API_CONFIG = {
    baseURL: 'https://localhost:7145/swagger/index.html',
    timeout: 10000 // 10 segundos de timeout
};

// CLASE PARA MANEJAR LA CONEXIÓN CON LA API
class ApiConnection {
    constructor() {
        this.baseURL = API_CONFIG.baseURL;
        this.timeout = API_CONFIG.timeout;
    }

    // Método principal para hacer peticiones HTTP
    async makeRequest(endpoint, options = {}) {
        const url = `${this.baseURL}${endpoint}`;
        
        // Configuración CORS completa
        const requestConfig = {
            method: options.method || 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                // headers adicionales si es necesario
                ...options.headers
            },
            // Credenciales para CORS (si usas cookies/auth)
            credentials: 'include',
            // Modo CORS explícito
            mode: 'cors',
            ...options
        };

        // Agregar token de autorización si existe
        const token = localStorage.getItem('authToken');
        if (token) {
            requestConfig.headers['Authorization'] = `Bearer ${token}`;
        }

        // Agregar body si es POST/PUT
        if (options.data && ['POST', 'PUT', 'PATCH'].includes(requestConfig.method)) {
            requestConfig.body = JSON.stringify(options.data);
        }

        try {
            console.log(`🔄 ${requestConfig.method} ${url}`);
            
            // Hacer la petición con timeout
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), this.timeout);
            
            requestConfig.signal = controller.signal;
            
            const response = await fetch(url, requestConfig);
            
            clearTimeout(timeoutId);
            
            console.log(` Respuesta: ${response.status} ${response.statusText}`);

            //  Verificar si la respuesta es exitosa
            if (!response.ok) {
                // Manejar diferentes tipos de errores HTTP
                let errorMessage = `Error ${response.status}: ${response.statusText}`;
                
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorData.error || errorMessage;
                } catch {
                    // Si no se puede parsear el error, usar el mensaje por defecto
                }
                
                throw new Error(errorMessage);
            }

            //  Parsear respuesta JSON si tiene contenido
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const data = await response.json();
                console.log(' Datos recibidos:', data);
                return data;
            }
            
            return null;

        } catch (error) {
            console.error(' Error en petición:', error);
            
            //  Manejo específico de errores CORS
            if (error.name === 'TypeError' && error.message.includes('fetch')) {
                throw new Error('Error de conexión: Verifica que el backend esté ejecutándose y CORS esté configurado correctamente');
            }
            
            //  Manejo de timeout
            if (error.name === 'AbortError') {
                throw new Error('Timeout: El servidor tardó demasiado en responder');
            }
            
            throw error;
        }
    }

    // GET - Obtener datos
    async get(endpoint) {
        return this.makeRequest(endpoint, { method: 'GET' });
    }

    //  POST - Crear datos
    async post(endpoint, data) {
        return this.makeRequest(endpoint, {
            method: 'POST',
            data: data
        });
    }

    // PUT - Actualizar datos
    async put(endpoint, data) {
        return this.makeRequest(endpoint, {
            method: 'PUT',
            data: data
        });
    }

    //  DELETE - Eliminar datos
    async delete(endpoint) {
        return this.makeRequest(endpoint, { method: 'DELETE' });
    }

    //  Probar conexión con el backend
    async testConnection() {
        try {
            console.log('🔍 Probando conexión con backend...');
            
            //  Hacer una petición simple de prueba
            const response = await fetch(`${this.baseURL}/health`, {
                method: 'GET',
                mode: 'cors',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                console.log(' Backend conectado correctamente');
                return true;
            } else {
                console.warn(` Backend responde con: ${response.status}`);
                return false;
            }
            
        } catch (error) {
            console.error(' Error conectando con backend:', error.message);
            
            //  Mostrar mensaje específico según el error
            if (error.message.includes('CORS')) {
                console.error(' Solución: Configura CORS en tu backend .NET');
            } else if (error.message.includes('fetch')) {
                console.error(' Solución: Verifica que el backend');
            }
            
            return false;
        }
    }
}

//  INSTANCIA GLOBAL DE LA CONEXIÓN API
const apiConnection = new ApiConnection();

//  FUNCIONES ESPECÍFICAS PARA TU APLICACIÓN
// ================================

//  PRODUCTOS
async function cargarProductos() {
    try {
        const productos = await apiConnection.get('/producto');
        console.log(' Productos cargados:', productos);
        return productos || [];
    } catch (error) {
        console.error('Error cargando productos:', error.message);
        throw error;
    }
}

async function crearProducto(productoData) {
    try {
        const nuevoProducto = await apiConnection.post('/producto', productoData);
        console.log(' Producto creado:', nuevoProducto);
        return nuevoProducto;
    } catch (error) {
        console.error('Error creando producto:', error.message);
        throw error;
    }
}

async function eliminarProducto(id) {
    try {
        await apiConnection.delete(`/producto/${id}`);
        console.log(` Producto ${id} eliminado`);
        return true;
    } catch (error) {
        console.error('Error eliminando producto:', error.message);
        throw error;
    }
}

// COMPRAS
async function cargarCompras() {
    try {
        const compras = await apiConnection.get('/compra');
        console.log('Compras cargadas:', compras);
        return compras || [];
    } catch (error) {
        console.error('Error cargando compras:', error.message);
        throw error;
    }
}

async function crearCompra(compraData) {
    try {
        const nuevaCompra = await apiConnection.post('/compra', compraData);
        console.log(' Compra creada:', nuevaCompra);
        return nuevaCompra;
    } catch (error) {
        console.error('Error creando compra:', error.message);
        throw error;
    }
}

//  VENTAS
async function cargarVentas() {
    try {
        const ventas = await apiConnection.get('/venta');
        console.log(' Ventas cargadas:', ventas);
        return ventas || [];
    } catch (error) {
        console.error('Error cargando ventas:', error.message);
        throw error;
    }
}

async function crearVenta(ventaData) {
    try {
        const nuevaVenta = await apiConnection.post('/venta', ventaData);
        console.log(' Venta creada:', nuevaVenta);
        return nuevaVenta;
    } catch (error) {
        console.error('Error creando venta:', error.message);
        throw error;
    }
}

// KARDEX
async function cargarKardex(productoId) {
    try {
         const kardex = await apiConnection.get(`/producto/${productoId}/kardex`);
        console.log(` Kardex del producto ${productoId}:`, kardex);
        return kardex || [];
    } catch (error) {
        console.error('Error cargando kardex:', error.message);
        throw error;
    }
}

//  AUTENTICACIÓN (si la tienes)
async function login(credentials) {
    try {
        const response = await apiConnection.post('/auth/login', credentials);
        
        if (response && response.token) {
            localStorage.setItem('authToken', response.token);
            console.log(' Login exitoso');
            return response;
        }
        
        throw new Error('Credenciales inválidas');
    } catch (error) {
        console.error('Error en login:', error.message);
        throw error;
    }
}

function logout() {
    localStorage.removeItem('authToken');
    console.log(' Logout exitoso');
}

//  INICIALIZACIÓN
document.addEventListener('DOMContentLoaded', async function() {
    console.log(' Iniciando conexión con backend...');
    
    //  Probar conexión al cargar la página
    const connected = await apiConnection.testConnection();
    
    if (connected) {
        console.log(' Sistema listo para usar');
        
        //  Aquí puedes cargar tus datos iniciales
        try {
            await Promise.all([
                cargarProductos(),
                cargarCompras(),
                cargarVentas()
            ]);
            console.log(' Todos los datos cargados exitosamente');
        } catch (error) {
            console.error(' Error cargando datos iniciales:', error.message);
        }
        
    } else {
        console.error(' No se pudo conectar con el backend');
        
        //  Mostrar mensaje al usuario
        alert(' No se pudo conectar con el servidor.\n\nVerifica que:\n• El backend esté ejecutándose\n• CORS esté configurado correctamente\n• La URL sea la correcta');
    }
});

//  FUNCIÓN DE UTILIDAD PARA MANEJAR ERRORES EN LA UI
function mostrarError(mensaje) {
    console.error('UI Error:', mensaje);
    
    //  Mostrar toast de error (si tienes Bootstrap)
    if (typeof bootstrap !== 'undefined') {
        // Crear y mostrar toast
        const toastHtml = `
            <div class="toast align-items-center text-bg-danger border-0 position-fixed top-0 end-0 m-3" style="z-index: 9999;">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fas fa-exclamation-circle me-2"></i>${mensaje}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;
        
        document.body.insertAdjacentHTML('beforeend', toastHtml);
        const toast = document.querySelector('.toast:last-child');
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
        
        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    } else {
        //  Fallback con alert si no hay Bootstrap
        alert(`Error: ${mensaje}`);
    }
}

function mostrarExito(mensaje) {
    console.log('UI Success:', mensaje);
    
    if (typeof bootstrap !== 'undefined') {
        const toastHtml = `
            <div class="toast align-items-center text-bg-success border-0 position-fixed top-0 end-0 m-3" style="z-index: 9999;">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fas fa-check-circle me-2"></i>${mensaje}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;
        
        document.body.insertAdjacentHTML('beforeend', toastHtml);
        const toast = document.querySelector('.toast:last-child');
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
        
        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    }
    async function actualizarDashboardAPI() {
            try {
                const stats = await API.dashboard.getStats();
                
                document.getElementById('total-productos').textContent = stats.totalProductos;
                document.getElementById('total-compras').textContent = stats.totalCompras;
                document.getElementById('total-ventas').textContent = stats.totalVentas;
                document.getElementById('productos-stock-bajo').textContent = stats.productosStockBajo;

                // Agregar información adicional al dashboard
                const dashboardSection = document.getElementById('dashboard-section');
                let existingChart = dashboardSection.querySelector('.resumen-mes-container');
                
                if (!existingChart) {
                    dashboardSection.innerHTML += `
                        <div class="row mt-4 resumen-mes-container">
                            <div class="col-md-6">
                                <div class="card">
                                    <div class="card-header">
                                        <h5><i class="fas fa-chart-line"></i> Resumen del Mes</h5>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-6">
                                                <div class="text-center">
                                                    <h4 class="text-success" id="ventas-mes-total">S/ ${stats.ventasDelMes.toFixed(2)}</h4>
                                                    <small class="text-muted">Ventas del Mes</small>
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="text-center">
                                                    <h4 class="text-danger" id="compras-mes-total">S/ ${stats.comprasDelMes.toFixed(2)}</h4>
                                                    <small class="text-muted">Compras del Mes</small>
                                                </div>
                                            </div>
                                        </div>
                                        <hr>
                                        <div class="text-center">
                                            <h4 class="${stats.ventasDelMes - stats.comprasDelMes >= 0 ? 'text-success' : 'text-danger'}" id="ganancia-mes-total">
                                                S/ ${(stats.ventasDelMes - stats.comprasDelMes).toFixed(2)}
                                            </h4>
                                            <small class="text-muted">Ganancia/Pérdida del Mes</small>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="card">
                                    <div class="card-header">
                                        <h5><i class="fas fa-exclamation-triangle"></i> Alertas de Stock</h5>
                                    </div>
                                    <div class="card-body">
                                        <div id="alertas-stock">
                                            <!-- Se cargarán las alertas aquí -->
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                } else {
                    // Actualizar valores existentes
                    const ventasMesElement = document.getElementById('ventas-mes-total');
                    const comprasMesElement = document.getElementById('compras-mes-total');
                    const gananciaMesElement = document.getElementById('ganancia-mes-total');
                    
                    if (ventasMesElement) ventasMesElement.textContent = `S/ ${stats.ventasDelMes.toFixed(2)}`;
                    if (comprasMesElement) comprasMesElement.textContent = `S/ ${stats.comprasDelMes.toFixed(2)}`;
                    if (gananciaMesElement) {
                        const ganancia = stats.ventasDelMes - stats.comprasDelMes;
                        gananciaMesElement.textContent = `S/ ${ganancia.toFixed(2)}`;
                        gananciaMesElement.className = ganancia >= 0 ? 'text-success' : 'text-danger';
                    }
                }

                // Actualizar alertas de stock
                const alertasContainer = document.getElementById('alertas-stock');
                if (alertasContainer) {
                    const productosStockBajo = productos.filter(p => p.stock < 10);
                    alertasContainer.innerHTML = '';
                    
                    if (productosStockBajo.length === 0) {
                        alertasContainer.innerHTML = '<p class="text-success"><i class="fas fa-check-circle"></i> Todos los productos tienen stock suficiente</p>';
                    } else {
                        productosStockBajo.forEach(producto => {
                            alertasContainer.innerHTML += `
                                <div class="alert alert-warning alert-sm mb-2">
                                    <strong>${producto.nombre}</strong> - Stock: ${producto.stock}
                                </div>
                            `;
                        });
                    }
                }

            } catch (error) {
                mostrarMensaje('Error al cargar dashboard: ' + error, 'danger');
            }
        }

}