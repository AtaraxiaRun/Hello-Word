pipeline {
    agent any
    
    stages {
    stage('Build') {
        steps {
            // 在此处进行您的构建操作，生成镜像
            // 例如，使用 Dockerfile 构建您的项目镜像
            sh 'docker build -t consoleapp6-image:1.0 .'
        }
    }

    stage('Publish') {
        steps {
            script {
                // 登录到 Docker Hub
                sh 'docker login -u libaibaihi -p qSNz9ofsnKb00A4c'

                // 标记镜像
                sh 'docker tag consoleapp6-image:1.0 libaibaihi/consoleapp6:1.0'

                // 推送镜像到 Docker Hub
                sh 'docker push libaibaihi/consoleapp6:1.0'
            }
        }
    }

}

}
